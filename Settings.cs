using System;

namespace invitational {
    class Settings {
        public static Settings instance = null; 

        // Game Settings
        public int maxPlayers;

        // Bot Settings
        private string Token; 
        public char commandPrefix;

        public string queueImage;
        public string gameImage;
        public string winImage; 
        public string mapImage;

        public string[] maps;
        public int gameType; 

        private Settings() {
            if(instance == null) {
                Settings.instance = this; 
            }
        }

        public static Settings Load(SettingsFile values) {
            Settings settings = new Settings();
            settings.Token = values.token;
            settings.maxPlayers = values.maxPlayers;
            settings.commandPrefix = values.commandPrefix;

            settings.queueImage = values.queueImage;
            settings.gameImage = values.gameImage;
            settings.winImage = values.winImage;
            settings.maps = values.maps;

            settings.gameType = values.gameMode;
            settings.mapImage = values.mapImage;

            return Settings.instance;
        }

        public static string GetDiscordToken() {
            return instance.Token;
        }

    }
}