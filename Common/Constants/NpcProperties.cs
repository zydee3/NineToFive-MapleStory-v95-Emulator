namespace NineToFive.Constants {
    public static class NpcProperties {
        public const SpeakerType DefaultSpeakerType = SpeakerType.Npc;
        public const SpeakerOrientation DefaultSpeakerOrientation = SpeakerOrientation.NpcFaceLeft;

        public enum SpeakerType : byte {
            NpcEsc = 1,
            Player = 2,
            Npc = 4,
            Unknown2 = 8,
        }

        /// <summary>
        /// This is called bParam under CScriptMan::OnSay
        /// </summary>
        public enum SpeakerOrientation : byte {
            NpcFaceLeft = 0,
            NpcFaceRight = 1,
            UserFaceLeft = 2,
            UserFaceRight = 3,
        }

        public enum ScriptMessageType : byte {
            OnSay = 0,
            OnSayImage = 1,
            OnAskYesNo = 2,
            OnAskText = 3,
            OnAskNumber = 4,
            OnAskMenu = 5,
            OnAskQuiz = 6,
            OnAskSpeedQuiz = 7,
            OnAskAvataar = 8,
            OnAskMembershopAvatar = 9,
            OnAskPet = 10,
            OnAskPetAll = 11,
            OnAskBoxText = 14,
            OnAskSlideMenu = 15,
        }

        public enum ScriptActionType : int { }
    }
}