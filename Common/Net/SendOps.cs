// CInPacket | On prefix

namespace NineToFive.SendOps {
    public enum CAffectedAreaPool : int {
        OnAffectedAreaCreated               = 328,
        OnAffectedAreaRemoved               = 329,
        CAdminShopDlgOnPacket_a1            = 366
    }

    public enum CBattleRecordMan : int {
        OnDotDamageInfo                     = 421,
        OnServerOnCalcRequestResult         = 422
    }

    public enum CCashShop : int {
        OnChargeParamResult                 = 382,
        OnQueryCashResult                   = 383,
        OnCashItemResult                    = 384,
        OnPurchaseExpChanged                = 385,
        OnGiftMateInfoResult                = 386,
        OnCheckDuplicatedIDResult           = 387,
        OnCheckNameChangePossibleResult     = 388
    }

    public enum CClientSocket : int {
        OnMigrateCommand        = 16,
        OnAliveReq              = 17,
        OnAuthenCodeChanged     = 18,
        OnAuthenMessage         = 19,
        OnCheckCrcResult        = 23
    }
    
    public enum CCashTradingRoomDlg : int {
        OnPutItem                           =  15,
        OnPutMoney                          =  16,
        OnTrade                             =  17
    }

    public enum CDropPool : int {
        OnDropEnterField                    = 322,
        OnDropLeaveField                    = 324
    }

    public enum CEmployeePool : int {
        OnEmployeeEnterField                = 319,
        OnEmployeeLeaveField                = 320,
        OnEmployeeMiniRoomBalloon           = 321
    }

    public enum CEntrustedShopDlg : int {
        OnArrangeItemResult                 =  40,
        OnWithdrawAllResult                 =  42,
        OnWithdrawMoneyResult               =  44,
        OnVisitListResult                   =  46,
        OnBlackListResult                   =  47
    }

    public enum CField_AriantArena : int {
        OnShowResult                        = 171,
        OnUserScore                         = 354
    }

    public enum CField_Battlefield : int {
        OnScoreUpdate                       = 356,
        OnTeamChanged                       = 357
    }

    public enum CField_Coconut : int {
        OnCoconutHit                        = 342,
        OnCoconutScore                      = 343
    }

    public enum CField_ContiMove : int {
        OnContiMove                         = 164,
        OnContiState                        = 165
    }

    public enum CField_GuildBoss : int {
        OnHealerMove                        = 344,
        OnPulleyStateChange                 = 345
    }

    public enum CField_KillCount : int {
        OnKillCountInfo                     = 178
    }
    
    public enum CField_Massacre : int {
        OnMassacreIncGauge                  = 173
    }

    public enum CField_MassacreResult : int {
        OnMassacreResult                    = 174
    }

    public enum CField_QuickslotKeyMappedMan : int {
        OnInit = 175
    }
    
    public enum CField_MonsterCarnivalRevive : int {
        OnEnter                             = 346,
        OnShowGameResult                    = 353
    }

    public enum CField_MonsterCarnival : int {
        OnEnter                             = 346,
        OnPersonalCP                        = 347,
        OnTeamCP                            = 348,
        OnRequestResult_Enable              = 349, // OnRequestResult(1, a4);
        OnRequestResult_Disable             = 350, // OnRequestResult(0, a4);
        OnProcessForDeath                   = 351,
        OnShowMemberOutMsg                  = 352,
        OnShowGameResult                    = 353
    }

    public enum CField_SnowBall : int {
        OnSnowBallState                     = 338,
        OnSnowBallHit                       = 339,
        OnSnowBallMsg                       = 340,
        OnSnowBallTouch                     = 341
    }

    public enum CField_Tournament : int {
        OnTournament                        = 374,
        OnTournamentMatchTable              = 375,
        OnTournamentSetPrize                = 376,
        OnTournamentUEW                     = 377,
        EMPTY                               = 378
    }

    public enum CField_Wedding : int {
        OnWeddingProgress                   = 379,
        OnWeddingCeremonyEnd                = 380
    }

    public enum CField_Witchtower : int {
        OnScoreUpdate                       = 358
    }

    public enum CField : int {
        OnTransferFieldReqIgnored           = 147,
        OnTransferChannelReqIgnored         = 148,
        OnFieldSpecificData                 = 149,
        OnGroupMessage                      = 150,
        OnWhisper                           = 151,
        OnCoupleMessage                     = 152,
        OnSummonItemInavailable             = 153,
        OnFieldEffect                       = 154,
        OnFieldObstacleOnOff                = 155,
        OnFieldObstacleOnOffStatus          = 156,
        OnFieldObstacleAllReset             = 157,
        OnBlowWeather                       = 158,
        OnPlayJukeBox                       = 159,
        OnAdminResult                       = 160,
        OnQuiz                              = 161,
        OnDesc                              = 162,
        UNKNOWN                             = 163,
        OnSetQuestClear                     = 166,
        OnSetQuestTime                      = 167,
        OnWarnMessage                       = 168,
        OnSetObjectState                    = 169,
        OnDestroyClock                      = 170,
        OnStalkResult                       = 172,
        OnFootHoldInfo                      = 176,
        OnRequestFootHoldInfo               = 177,
        EMPTY                               = 196,
        // case 359: CField::OnHontailTimer(Format)
        // case 361: CField::OnHontailTimer(Format)
        OnHontaleTimer                      = 359,
        OnChaosZakumTimer                   = 360,
        OnZakumTimer                        = 362
    }

    public enum CFuncKeyMappedMan : int {
        OnInit                              = 398,
        OnPetConsumeItemInit                = 399,
        OnPetConsumeMPItemInit              = 400,
    }

    public enum CITC : int {
        OnChargeParamResult                 = 410,
        OnQueryCashResult                   = 411,
        OnNormalItemResult                  = 412
    }

    public enum CLogin : int {  
        OnCheckPasswordResult               =   0,
        OnGuestIDLoginResult                =   1,
        OnAccountInfoResult                 =   2,
        OnCheckUserLimitResult              =   3,
        OnSetAccountResult                  =   4,
        OnConfirmEULAResult                 =   5,
        OnCheckPinCodeResult                =   6,
        OnUpdatePinCodeResult               =   7,
        OnViewAllCharResult                 =   8,
        OnSelectCharacterByVACResult        =   9,
        OnWorldInformation                  =  10,
        OnSelectWorldResult                 =  11,
        OnSelectCharacterResult             =  12,
        OnCheckDuplicatedIDResult           =  13,
        OnCreateNewCharacterResult          =  14,
        OnDeleteCharacterResult             =  15,
        OnEnableSPWResult                   =  21,
        OnLatestConnectedWorld              =  24,
        OnRecommendWorldMessage             =  25,
        OnExtraCharInfoResult               =  26,
        OnCheckSPWResult                    =  27,
    }

    public enum CMapLoadable : int {
        OnSetBackEffect                     = 144,
        OnSetMapObjectVisible               = 145,
        OnClearBackEffect                   = 146
    }

    public enum CMapleTVMan : int {
        OnSetMessage                        = 405,
        OnClearMessage                      = 406,
        OnSendMessageResult                 = 407
    }

    public enum CMemoryGameDlg : int {
        OnTieRequest                        =  50,
        OnTieResult                         =  51,
        OnUserReady                         =  58,
        OnUserCancelReady                   =  59,
        OnUserStart                         =  61,
        OnGameResult                        =  62,
        OnTimeOver                          =  63,
        OnTurnUpCard                        =  68
    }

    public enum CMessageBoxPool : int {
        OnCreateFailed                      = 325,
        OnMessageBoxEnterField              = 326,
        OnMessageBoxLeaveField              = 327
    }

    public enum CMiniRoomBaseDlg : int {

    }

    public enum CMiniRoomBaseDlg_onPacketBase : int {
        OnInviteStatic                      =   2,
        OnInviteResultStatic                =   3,
        OnEnterResultStatic                 =   5,
        OnCheckSSN2Static                   =  14
    }

    public enum CMob : int {
        OnMove                              = 287,
        OnCtrlAck                           = 288,
        OnStatSet                           = 290,
        OnStatReset                         = 291,
        OnSuspendReset                      = 292,
        OnAffected                          = 293,
        OnDamaged                           = 294,
        OnSpecialEffectBySkill              = 295,
        OnHPIndicator                       = 298,
        OnCatchEffect                       = 299,
        OnEffectByItem                      = 300,
        OnMobSpeaking                       = 301,
        OnIncMobChargeCount                 = 302,
        OnMobSkillDelay                     = 303,
        OnEscortFullPat                     = 304,
        OnEscortStopSay                     = 306,
        OnEscortReturnBefore                = 307,
        OnNextAttack                        = 308,
        OnMobAttackedByMob                  = 309
    }

    public enum CMobPool : int {
        OnMobEnterField                     = 284,
        OnMobLeaveField                     = 285,
        OnMobChangeController               = 286,
        OnMobCrcKeyChanged                  = 297
    }

    public enum CNpc : int {
        OnMove                              = 314,
        OnUpdateLimitedInfo                 = 315,
        OnSetSpecialAction                  = 316
    }
    public enum CNpcPool : int {
        OnNpcImitateData                    =  84,
        OnUpdateLimitedDisableInfo          =  85,
        OnNpcEnterField                     = 311,
        OnNpcLeaveField                     = 312,
        OnNpcChangeController               = 313
    }

    public enum CNpcTemplate : int {
        CNpcTemplate                        = 317
    }

    public enum COmokDlg : int {
        OnTieRequest                        =  50,
        OnTieResult                         =  51,
        OnRetreatRequest                    =  54,
        OnRetreatResult                     =  55,
        OnUserReady                         =  58,
        OnUserCancelReady                   =  59,
        OnUserStart                         =  61,
        OnGameResult                        =  62,
        OnTimeOver                          =  63,
        OnPutStoneChecker                   =  64,
        OnPutStoneCheckerErr                =  65
    }

    public enum COpenGatePool : int {
        OnOpenGateCreated                   = 332,
        OnOpenGateRemoved                   = 333
    }

    public enum CParcelDlg : int {
    
    }

    public enum CPersonalShopDlg : int {

    }

    public enum CReactorPool : int {
        OnReactorChangeState                = 334,
        OnReactorMove                       = 335,
        OnReactorEnterField                 = 336,
        OnReactorLeaveField                 = 337
    }

    public enum CScriptMan : int {
        OnScriptMessage                     = 363
    }

    public enum CSecurityClient : int {
        SetShopDlg                          = 364,
        EMPTY                               = 365
    }

    public enum CStage : int {
        OnSetField                          = 141,
        OnSetITC                            = 142,
        OnSetCashShop                       = 143
    }

    public enum CStoreBankDlg : int {
        EMPTY                               = 369,
    }

    public enum CSummonedPool : int {
        OnCreated                           = 278,
        OnRemoved                           = 279,
        OnMove                              = 280,
        OnAttack                            = 281,
        OnSkill                             = 282,
        OnHit                               = 283
    }

    public enum CTownPortalPool : int {
        OnTownPortalCreated                 = 330,
        OnTownPortalRemoved                 = 331
    }

    public enum CTradingRoomDlg : int {

    }

    public enum CTrunkDlg : int {

    }

    public enum CUICharacterSaleDlg : int {
        OnCheckDuplicatedIDResult           = 431,
        OnCreateNewCharacterResult          = 414
    }

    public enum CUIItemUpgrade : int {
        OnItemUpgradeResult                 = 425
    }

    public enum CUIMessenger : int { 

    }

    public enum CUIPartySearch : int {

    }

    public enum TabPartyAdver : int {

    }

    public enum CUIVega : int {
        OnVegaResult                        = 429
    }

    public enum CUser : int {
        OnChat_Send                         = 181, // OnChat(v7, a3, 0)
        OnChat_Recv                         = 182, // OnChat(v7, a3, 1)
        OnADBoard                           = 183,
        OnMiniRoomBalloon                   = 184,
        SetConsumeItemEffect_0              = 185,
        ShowItemUpgradeEffect               = 186,
        ShowItemHyperUpgradeEffect          = 187,
        ShowItemOptionUpgradeEffect         = 188,
        ShowItemReleaseEffect               = 189,
        ShowItemUnreleaseEffect             = 190,
        OnHitByUser                         = 191,
        OnTeslaTriangle                     = 192,
        OnFollowCharacter                   = 193,
        OnShowPQReward                      = 194,
        OnSetPhase                          = 193,
        ShowRecoverUpgradeCountEffect       = 197,
    }

    public enum CUserLocal : int {
        OnSitResult                         = 231,
        OnEmotion                           = 232,
        OnEffect                            = 233,
        OnTeleport                          = 234,
        OnMesoGive_Succeeded                = 236,
        OnMesoGive_Failed                   = 237,
        OnRandomMesobag_Succeeded           = 238,
        OnRandomMesobag_Failed              = 239,
        OnFieldFadeInOut                    = 240,
        OnFieldFadeOutForce                 = 241,
        OnQuestResult                       = 242,
        OnNotifyHPDecByField                = 243,
        OnBalloonMsg                        = 245,
        OnPlayEventSound                    = 246,
        OnPlayMinigameSound                 = 247,
        OnMakerResult                       = 248,
        OnOpenClassCompetitionPage          = 250,
        OnOpenUI                            = 251,
        OnOpenUIWithOption                  = 252,
        OnSetDirectionMode                  = 253,
        OnSetStandAloneMode                 = 254,
        OnHireTutor                         = 255,
        OnTutorMsg                          = 256,
        OnIncComboResponse                  = 257,
        OnRandomEmotion                     = 258,
        OnResignQuestReturn                 = 259,
        OnPassMateName                      = 260,
        OnRadioSchedule                     = 261,
        OnOpenSkillGuide                    = 262,
        OnNoticeMsg                         = 263,
        OnChatMsg                           = 264,
        OnBuffzoneEffect                    = 265,
        OnGoToCommoditySN                   = 266,
        OnDamageMeter                       = 267,
        OnTimeBombAttack                    = 268,
        OnPassiveMove                       = 269,
        OnFollowCharacterFailed             = 270,
        OnVengeanceSkillApply               = 271,
        OnExJablinApply                     = 272,
        OnAskAPSPEvent                      = 273,
        OnQuestGuideResult                  = 274,
        OnDeliveryQuest                     = 275,
        OnSkillCooltimeSet                  = 276
    }

    public enum CUserPool : int {
        OnUserEnterField                    = 179,
        OnUserLeaveField                    = 180,

    }

    public enum CUserRemote : int {
        OnMove                              = 210,
        OnMeleeAttack                       = 211,
        OnShootAttack                       = 212,
        OnMagicAttack                       = 213,
        OnBodyAttack                        = 214,
        OnSkillPrepare                      = 215,
        OnMovingShootAttackPrepare          = 216,
        OnSkillCancel                       = 217,
        OnHit                               = 218,
        OnEmotion                           = 219,
        OnSetActiveEffectItem               = 220,
        OnShowUpgradeTombEffect             = 221,
        OnSetActivePortableChair            = 222,
        OnAvatarModified                    = 223,
        OnEffect                            = 224,
        OnResetTemporaryStat                = 226,
        OnReceiveHP                         = 227,
        OnGuildNameChanged                  = 228,
        OnGuildMarkChanged                  = 229,
        OnSetTemporaryStat                  = 255,
        OnThrowGrenade                      = 230
    }

    public enum CPet : int {
        OnMove                              = 201,
        OnAction                            = 202,
        OnNameChanged                       = 203,
        OnLoadExceptionList                 = 204,
        OnActionCommand                     = 205
    }

    public enum CDragon : int {
        OnMove                              = 207
    }

    public enum CWishListGiveDlg : int {

    }

    public enum CWishListRecvDlg : int {

    }

    public enum CWvsContext : int {
        OnInventoryOperation                =  28,
        OnInventoryGrow                     =  29,
        OnStatChanged                       =  30,
        OnTemporaryStatSet                  =  31,
        OnTemporaryStatReset                =  32,
        OnForcedStatSet                     =  33,
        OnForcedStatReset                   =  34,
        OnChangeSkillRecordResult           =  35,
        OnSkillUseResult                    =  36,
        OnGivePopularityResult              =  37,
        OnMessage                           =  38,
        OnOpenFullClientDownloadLink        =  39,
        OnMemoResult                        =  40,
        OnMapTransferResult                 =  41,
        OnAntiMacroResult                   =  42,
        OnClaimResult                       =  44, 
        OnSetClaimSvrAvailableTime          =  45,
        OnClaimSvrStatusChanged             =  46,
        OnSetTamingMobInfo                  =  47,
        OnQuestClear                        =  48,
        OnEntrustedShopCheckResult          =  49,
        OnSkillLearnItemResult              =  51,
        OnSkillResetItemResult              =  52,
        OnGatherItemResult                  =  52,
        OnSortItemResult                    =  53,
        OnSueCharacterResult                =  55,
        OnTradeMoneyLimit                   =  57,
        OnSetGender                         =  58,
        OnGuildBBSPacket                    =  59,
        OnCharacterInfo                     =  61,
        OnPartyResult                       =  62,
        OnExpedtionResult                   =  64, 
        OnFriendResult                      =  65,
        OnGuildResult                       =  67,
        OnAllianceResult                    =  68,
        OnTownPortal                        =  69,
        OnOpenGate                          =  70,
        OnBroadcastMsg                      =  71,
        OnIncubatorResult                   =  72,
        OnShopScannerResult                 =  73,
        OnShopLinkResult                    =  74,
        OnMarriageRequest                   =  75,
        OnMarriageResult                    =  76, 
        OnWeddingGiftResult                 =  77,
        OnNotifyMarriedPartnerMapTransfer   =  78,
        OnCashPetFoodResult                 =  79,
        OnSetWeekEventMessage               =  80,
        OnSetPotionDiscountRate             =  81,
        OnBridleMobCatchFail                =  82,
        OnImitatedNPCResult                 =  83,
        OnImitatedNPCData                   =  84,
        OnLimitedNPCDisableInfo             =  85,
        OnMonsterBookSetCard                =  86,
        OnMonsterBookSetCover               =  87,
        OnHourChanged                       =  88,
        OnMiniMapOnOff                      =  89,
        OnConsultAuthkeyUpdate              =  90,
        OnClassCompetitionAuthkeyUpdate     =  91,
        OnWebBoardAuthkeyUpdate             =  92,
        OnSessionValue                      =  93,
        OnPartyValue                        =  94,
        OnFieldSetVariable                  =  95,
        OnBonusExpRateChanged               =  96,
        OnPotionDiscountRateChanged         =  97,
        OnFamilyChartResult                 =  98,
        OnFamilyInfoResult                  =  99,
        OnFamilyResult                      = 100,
        OnFamilyJoinRequest                 = 101,
        OnFamilyJoinRequestResult           = 102,
        OnFamilyJoinAccepted                = 103,
        OnFamilyPrivilegeList               = 104,
        OnFamilyFamousPointIncResult        = 105,
        OnFamilyNotifyLoginOrLogout         = 106,
        OnFamilySetPrivilege                = 107,
        OnFamilySummonRequest               = 108,
        OnNotifyLevelUp                     = 109,
        OnNotifyWedding                     = 110,
        OnNotifyJobChange                   = 111,
        OnMapleTVUseRes                     = 113,
        OnAvatarMegaphoneRes                = 114,
        OnSetAvatarMegaphone                = 115,
        OnClearAvatarMegaphone              = 116,
        OnCancelNameChangeResult            = 117,
        OnCancelTransferWorldResult         = 118,
        OnDestroyShopResult                 = 119,
        OnFakeGMNotice                      = 120,
        OnSuccessInUsegachaponBox           = 121,
        OnNewYearCardRes                    = 122,
        OnRandomMorphRes                    = 123,
        OnCancelNameChangebyOther           = 124,
        OnSetBuyEquipExt                    = 125,
        OnSetPassenserRequest               = 126,
        OnScriptProgressMessage             = 127,
        OnDataCRCCheckFailed                = 128,
        OnCakePieEventResult                = 129,
        OnUpdateGMBoard                     = 130,
        OnShowSlotMessage                   = 131,
        OnWildHunterInfo                    = 132,
        OnAccountMoreInfo                   = 133,
        OnFindFirend                        = 134,
        OnStageChange                       = 135,
        OnDragonBallBox                     = 136,
        OnAskWhetherUsePamsSong             = 137,
        OnTransferChannel                   = 138,
        OnDisallowedDeliveryQuestList       = 139,
        OnMacroSysDataInit                  = 140
    }
}
