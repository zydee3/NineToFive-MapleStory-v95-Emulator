namespace NineToFive.Constants {
    public static class GameConstants {
        public static class FieldConstants {
            public static bool IsTown(int fieldId) {
                switch (fieldId) {
                    case 100000000:
                    case 103000000:
                        return true;
                }

                return false;
            }
        }
    }
}