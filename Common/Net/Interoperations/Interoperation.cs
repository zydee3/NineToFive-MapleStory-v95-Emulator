namespace NineToFive.Net {
    public enum Interoperation : byte {
        /// <summary>
        /// When the channel server is requesting the login server for permission
        /// to create sockets for specific channels in world 
        /// </summary>
        ChannelHostRequest = 0,

        /// <summary>
        /// Login server request for world information (user count, message, events, etc.)
        /// </summary>
        WorldInformationRequest = 1,

        /// <summary>
        /// Login server request for <see cref="CLogin.OnCheckPasswordResult"/>
        /// </summary>
        CheckPasswordRequest = 2,

        /// <summary>
        /// Request for checking existing char usernames
        /// </summary>
        CheckDuplicateIdRequest = 3,

        /// <summary>
        /// Request for updating the account gender (occurs on first time login)
        /// </summary>
        ClientGenderUpdateRequest = 4,

        /// <summary>
        /// Request for initializing the account secondary password (occurs on first time char select) 
        /// </summary>
        ClientInitializeSPWRequest = 5,

        /// <summary>
        /// Request for migrating to a specified channel server
        /// </summary>
        MigrateClientRequest = 6,

        /// <summary>
        /// Request for updating the user count for a specified channel server
        /// </summary>
        ChannelUserLimitRequest = 7,
        ChannelUserLimitResponse = 8,
    }
}