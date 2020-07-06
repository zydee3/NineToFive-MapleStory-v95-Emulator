using System;
using System.Collections.Generic;
using log4net;
using MySql.Data.MySqlClient;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity;
using NineToFive.Game.Storage;
using Item = NineToFive.Game.Storage.Item;

namespace NineToFive.Util {
    public static class Database {
        public static DatabaseQuery Table(string table) {
            return new DatabaseQuery(table);
        }

        public static object[] CreateUserParameters(User user) {
            return new object[] {
                "account_id", user.AccountId,
                "username", user.CharacterStat.Username,
                "gender", user.AvatarLook.Gender,
                "skin", user.AvatarLook.Skin,
                "face", user.AvatarLook.Face,
                "hair", user.AvatarLook.Hair,
                "level", user.CharacterStat.Level,
                "job", user.CharacterStat.Job,
                "str", user.CharacterStat.Str,
                "dex", user.CharacterStat.Dex,
                "int", user.CharacterStat.Int,
                "luk", user.CharacterStat.Luk,
                "hp", user.CharacterStat.HP,
                "max_hp", user.CharacterStat.MaxHP,
                "mp", user.CharacterStat.MP,
                "max_mp", user.CharacterStat.MaxMP,
                "ability_points", user.CharacterStat.AP,
                "exp", user.CharacterStat.Exp,
                "popularity", user.CharacterStat.Popularity,
                "field_id", user.CharacterStat.FieldId,
                "portal", user.CharacterStat.Portal
            };
        }

        public static object[] CreateItemParameters(User user, Item item) {
            return new object[] {
                "account_id", user.AccountId,
                "character_id", user.CharacterStat.Id,
                "item_id", item.Id,
                "bag_index", item.BagIndex,
                "quantity", item.Quantity,
                "cash_sn", item.CashItemSn,
                "date_expire", item.DateExpire
            };
        }
    }

    enum QueryType {
        Undefined,
        Delete,
        Select,
        Insert,
        Update,
    }

    public class DatabaseQuery : IDisposable {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DatabaseQuery));
        private MySqlConnection Connection { get; set; }
        public MySqlCommand Command { get; set; }

        private readonly string _table;

        /// <summary>
        /// a list of columns to be selected
        /// </summary>
        private string _columns;

        /// <summary>
        /// behavior of the executing command (select, insert, delete or update)
        /// </summary>
        private QueryType _behavior = QueryType.Undefined;

        /// <summary>
        /// keys (column) to update in the database with specified values
        /// </summary>
        private List<object[]> _parameters;

        /// <summary>
        /// conditions that must be met for the executing command; a where statement
        /// </summary>
        private object[] _conditions;

        protected internal DatabaseQuery(string table) {
            _table = MySqlHelper.EscapeString(table);
        }

        public void Dispose() {
            Connection?.Dispose();
            Connection = null;
            Command?.Dispose();
            Command = null;
            _parameters?.Clear();
            _parameters = null;
        }

        /// <summary>
        /// Initializes a 'delete' execution statement.
        /// <code>"delete from table"...</code>
        /// </summary>
        /// <exception cref="InvalidOperationException">If method was called after another behavior call</exception>
        public DatabaseQuery Delete() {
            if (_behavior != QueryType.Undefined && _behavior != QueryType.Delete) throw new InvalidOperationException($"behavior is already: '{_behavior}'");
            _behavior = QueryType.Delete;
            return this;
        }

        /// <summary>
        /// Initializes a 'select' execution statement.
        /// <code>"select column1,column2,column3"...</code>
        /// <para>Example usage:</para>
        /// <code>select("column1,column2,column3")</code>
        /// </summary>
        /// <param name="columns">columns to select separated by a comma</param>
        /// <exception cref="InvalidOperationException">If method was called after another behavior call</exception>
        public DatabaseQuery Select(string columns = "*") {
            if (_behavior != QueryType.Undefined) throw new InvalidOperationException($"behavior is already: '{_behavior}'");

            _behavior = QueryType.Select;
            _columns = columns;
            return this;
        }

        /// <summary>
        /// Initializes an 'insert' execution statement.
        /// <code>"insert into table (column1, column2) values (value1,value2),(value3,value4)"...</code>
        /// <para>Example usage:</para>
        /// <code>Insert("column_name1", value1, "column_name2", value2)</code>
        /// <para>This method can be called repeatedly to build a number of insert statements that will be executed all-together</para>
        /// </summary>
        /// <param name="parameters">array of columns and values</param>
        /// <exception cref="InvalidOperationException">If method was called after another behavior call</exception>
        /// <exception cref="InvalidOperationException">If specified parameters is empty or null</exception>
        public DatabaseQuery Insert(params object[] parameters) {
            if (_behavior != QueryType.Undefined && _behavior != QueryType.Insert) throw new InvalidOperationException($"behavior is already: '{_behavior}'");
            if (parameters == null || parameters.Length == 0) throw new InvalidOperationException("cannot insert empty data");

            _behavior = QueryType.Insert;
            if (_parameters == null) {
                // initialize parameters container
                _parameters = new List<object[]>(2);
                // it's only natural to have an even number of parameters due to specifying columns and its value 
                if (parameters.Length % 2 != 0) throw new InvalidOperationException($"invalid amount of parameters to values");
            } else if (parameters.Length != _parameters[0].Length) {
                throw new InvalidOperationException($"invalid parameter count {parameters.Length}, should be {_parameters[0].Length}");
            }

            _parameters.Add(parameters);
            return this;
        }

        /// <summary>
        /// Initializes an 'update' execution statement.
        /// <code>"update table set column1=value1"...</code>
        /// <para>Example usage:</para>
        /// <code>Update("id", 15, "username", "new_username")</code>
        /// </summary>
        ///<exception cref="InvalidOperationException">If method was called after another behavior call</exception>
        /// <exception cref="InvalidOperationException">If specified parameters is empty or null</exception>
        public DatabaseQuery Update(params object[] parameters) {
            if (_behavior != QueryType.Undefined && _behavior != QueryType.Update) throw new InvalidOperationException($"behavior is already: '{_behavior}'");
            if (parameters == null || parameters.Length == 0) throw new InvalidOperationException("cannot update empty data");

            _behavior = QueryType.Update;
            _parameters ??= new List<object[]>(1);
            _parameters.Add(parameters);
            return this;
        }

        /// <summary>
        /// Each condition takes 3 elements, a column, the condition operator and the comparison value
        /// <para>Example usage which conditions an `id` column check between the values 15 and 20 (exclusive):</para> 
        /// <code>Where("id", <![CDATA[">"]]>, 15, "id", <![CDATA["<"]]> 20)</code>
        /// </summary>
        /// <exception cref="InvalidOperationException">If specified parameters is empty or null</exception>
        public DatabaseQuery Where(params object[] conditions) {
            if (conditions == null || conditions.Length == 0) throw new InvalidOperationException("cannot have empty conditions");
            _conditions = conditions;
            return this;
        }

        public int ExecuteNonQuery() {
            int count = Execute(out MySqlDataReader r);
            if (r != null) throw new InvalidOperationException("reader available for non-reader statement"); // reader should always be null
            return count;
        }

        public MySqlDataReader ExecuteReader() {
            Execute(out MySqlDataReader reader);
            return reader;
        }

        private int Execute(out MySqlDataReader reader) {
            EscapeParameters();

            switch (_behavior) {
                default: throw new InvalidOperationException($"Cannot execute behavior {_behavior}");
                case QueryType.Delete:
                case QueryType.Select: {
                    bool isReader = _behavior == QueryType.Select;
                    // create where statement
                    string query = ProcessConditions();
                    // execution behavior
                    if (isReader) {
                        query = $"select {_columns} from {_table}" + query;
                    } else {
                        query = $"delete from {_table}" + query;
                    }

                    (Connection = new MySqlConnection(ServerConstants.DatabaseConString)).Open();
                    Command = new MySqlCommand(query, Connection);

                    reader = isReader ? Command.ExecuteReader() : null;
                    return isReader ? 0 : Command.ExecuteNonQuery();
                }
                case QueryType.Insert: {
                    // get column names, all values share the same columns so we only need the first index
                    string columns = "(";
                    for (int i = 0; i < _parameters[0].Length; i += 2) {
                        columns += $"`{_parameters[0][i]}`,";
                    }

                    string query = $"insert into {_table} {columns.TrimEnd(',')}) values ";

                    // the idea is to have the ability to insert any number of rows (but never none)
                    // ...values (@value_0), (@value_1), (@value_2) and can continue to an unknown amount of times
                    int varCount = 0, totalVars = (_parameters[0].Length / 2) * _parameters.Count; // global variable count
                    foreach (object[] parameters in _parameters) {
                        string values = "(";
                        // local variable count (parameter indexer)
                        for (int i = 0; i < parameters.Length; i += 2) {
                            // append a concatenation of the parameter name and the insert index 
                            values += $"@{parameters[i]}_{varCount++},";
                        }

                        values = values.TrimEnd(',') + ")";
                        // append comma only if we are not inserting the last row
                        if (varCount < totalVars) values += ",";

                        query += values;
                    }

                    (Connection = new MySqlConnection(ServerConstants.DatabaseConString)).Open();
                    Command = new MySqlCommand(query, Connection);

                    varCount = 0; // reset to assign each declared variable
                    foreach (object[] parameters in _parameters) {
                        for (int i = 0; i < parameters.Length; i += 2) {
                            Command.Parameters.AddWithValue($@"{parameters[i]}_{varCount++}", parameters[i + 1]);
                        }
                    }

                    reader = null;
                    return Command.ExecuteNonQuery();
                }
                case QueryType.Update: {
                    if (_parameters.Count != 1) throw new InvalidOperationException("cannot update multiple parameters?");
                    string query = $"update {_table} set ";
                    object[] parameters = _parameters[0];
                    for (int i = 0; i < parameters.Length; i += 2) {
                        // column_name=@column_name
                        query += $"`{parameters[i]}`=@{parameters[i]}";
                    }

                    query += ProcessConditions();
                    (Connection = new MySqlConnection(ServerConstants.DatabaseConString)).Open();
                    Command = new MySqlCommand(query, Connection);
                    for (int i = 0; i < parameters.Length; i += 2) {
                        Command.Parameters.AddWithValue($@"{parameters[i]}", parameters[i + 1]);
                    }

                    reader = null;
                    return Command.ExecuteNonQuery();
                }
            }
        }

        private void EscapeParameters() {
            for (int i = 0; i < _conditions?.Length; i += 3) {
                if (_conditions[i + 2] is string input) {
                    _conditions[i + 2] = "'" + MySqlHelper.EscapeString(input) + "'";
                }
            }

            if (_columns != null) {
                string[] sp = _columns.Split(",");
                string columns = "";
                foreach (var s in sp) {
                    columns += $"`{s}`,";
                }

                _columns = columns.TrimEnd(',');
            }

            if (_parameters != null) {
                foreach (object[] parameters in _parameters) {
                    for (int i = 0; i < parameters.Length; i++) {
                        if (!(parameters[i] is string input)) continue;
                        parameters[i] = MySqlHelper.EscapeString(input);
                    }
                }
            }
        }

        private string ProcessConditions() {
            if (_conditions == null || _conditions.Length == 0) return "";

            string where = " where ";
            for (int i = 0; i < _conditions.Length; i++) {
                if (i % 3 != 0) continue;
                // [column] [operator] [variable]
                // example: [`id` < 10]
                where += $"`{_conditions[i]}` {_conditions[i + 1]} {_conditions[i + 2]} and ";
            }

            return where.TrimEnd(" and ".ToCharArray());
        }
    }
}