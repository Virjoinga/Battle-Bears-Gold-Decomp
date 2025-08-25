public static class ServerX
{
    public static string GetRootAddress(ServerAddressType typ)
    {
        /*switch (typ)
        {
            case ServerAddressType.DEVELOPMENT: return "http://127.0.0.1:6755";
            case ServerAddressType.STAGING: return "http://127.0.0.1:6755";
            case ServerAddressType.PRODUCTION: return "http://127.0.0.1:6755";
            default: return "http://127.0.0.1:6755";
        };*/
        return "http://127.0.0.1:6755";
    }

    public static string GetMatchmakingAddress(MatchingServerType typ)
    {   
        /*switch (typ)
        {
            case MatchingServerType.NONE: return "http://127.0.0.1:6755";
            case MatchingServerType.ONE: return "http://127.0.0.1:6755";
            case MatchingServerType.TWO: return "http://127.0.0.1:6755";
            case MatchingServerType.EIGHT: return "http://127.0.0.1:6755";
            default: return "http://127.0.0.1:6755";
        };*/
        return "http://127.0.0.1:6755";
    }
}
