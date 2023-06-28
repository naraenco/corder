namespace agentcs
{
    internal class PosData
    {
        public void LoadJsonFiles()
        {
            Config config = Config.Instance;

            JsonWrapper jsonTableMap = new();
            if (jsonTableMap.Load(config.GetString("path_tablemap"), codepage: 51949) == true)
            {
                jsonTableMap.SetOptions(false);
                jsonTableMap.Parse();
            }

            JsonWrapper jsonMenu = new();
            if (jsonMenu.Load(config.GetString("path_menu"), codepage: 51949) == true)
            {
                jsonMenu.SetOptions(false);
                jsonMenu.Parse();
                //jsonMenu.Save(@"c:\temp\test.json", indent: true, codepage: 51949);
            }
        }
    }
}
