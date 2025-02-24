namespace Game.Server.Managers
{
    using Bussiness;
    using SqlDataProvider.Data;
    using System;

    public class kethop
    {
        public static Items_Fusion_List_Info[] m_itemsfusionlist = null;
        public static KethopInfo[] m_listkethop = null;

        public static KethopInfo[] ListKethop()
        {
            using (ProduceBussiness bussiness = new ProduceBussiness())
            {
                m_itemsfusionlist = bussiness.GetAllFusionList();
            }
            return m_listkethop;
        }
    }
}

