namespace invitational {
    class Settings {
        public static Settings instance = null; 

        // Game Settings
        public int maxPlayers;
        public int maxTeams;

        // Bot Settings
        private string Token; 
        public char commandPrefix;

        private Settings() {
            if(instance == null) {
                Settings.instance = this; 
            }
        }

        public static Settings Load(SettingsFile values) {
            Settings settings = new Settings();
            settings.Token = values.token;
            settings.maxPlayers = values.maxPlayers;
            settings.maxTeams = values.maxTeams;
            settings.commandPrefix = values.commandPrefix;

            return Settings.instance;
        }

        public static string GetDiscordToken() {
            return instance.Token;
        }

    }
}