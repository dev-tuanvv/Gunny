namespace Game.Logic.Phy.Object
{
    using System;

    public class Layer : PhysicalObj
    {
        public Layer(int id, string name, string model, string defaultAction, int scale, int rotation) : base(id, name, model, defaultAction, scale, rotation, 0)
        {
        }

        public override int Type
        {
            get
            {
                return 2;
            }
        }
    }
}

