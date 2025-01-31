﻿namespace fnf.DocQuery.WebUI.Model
{
    /// <summary>
    ///     Stores data which needs to be present across pages.
    /// </summary>
    public class SharedDataModel
    {
        public static List<string> Messages { get; set; } = [];
        public static List<string> Responses { get; set; } = [];

        public static List<string> datetime { get; set; } = [];
        public static Dictionary<string,string> SharedFileList { get; set; } = [];

    }
}
