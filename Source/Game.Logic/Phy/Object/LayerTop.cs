namespace Game.Logic.Phy.Object
{
    using System;

    public class LayerTop : PhysicalObj
    {
        public LayerTop(int id, string name, string model, string defaultAction, int scale, int rotation) : base(id, name, model, defaultAction, scale, rotation, -1)
        {

        }

        public override int Type
        {
            get
            {
                return 0;
            }
        }
    }
}

