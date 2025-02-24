namespace Game.Logic.Phy.Object
{
    using Game.Logic;
    using Game.Logic.Actions;
    using System;
    using System.Collections.Generic;

    public class PhysicalObj : Physics
    {
        private Dictionary<string, string> m_actionMapping;
        private bool m_canPenetrate;
        private string m_currentAction;
        private BaseGame m_game;
        private string m_model;
        private string m_name;
        private int m_phyBringToFront;
        private int m_rotation;
        private int m_scale;
        private int m_type;
        private int m_typeEffect;

        public PhysicalObj(int id, string name, string model, string defaultAction, int scale, int rotation, int typeEffect) : base(id)
        {
            this.m_name = name;
            this.m_model = model;
            this.m_currentAction = defaultAction;
            this.m_scale = scale;
            this.m_rotation = rotation;
            this.m_canPenetrate = false;
            this.m_typeEffect = typeEffect;
            if (name != null)
            {
                if (name == "hide")
                {
                    this.m_phyBringToFront = 6;
                    goto Label_0087;
                }
                if (name == "top")
                {
                    this.m_phyBringToFront = 1;
                    goto Label_0087;
                }
            }
            this.m_phyBringToFront = -1;
        Label_0087:
            this.m_actionMapping = new Dictionary<string, string>();
            if (model == "asset.game.transmitted")
            {
                this.m_type = 3;
            }
            else if (model == "asset.game.six.ball")
            {
                if (!this.m_actionMapping.ContainsKey(defaultAction))
                {
                    this.m_actionMapping.Add(defaultAction, this.getActionMap(defaultAction));
                }
            }
            else
            {
                this.m_type = 0;
            }
        }

        public override void CollidedByObject(Physics phy)
        {
            if (!(this.m_canPenetrate || !(phy is SimpleBomb)))
            {
                ((SimpleBomb) phy).Bomb();
            }
        }

        private string getActionMap(string act)
        {
            switch (act)
            {
                case "s1":
                    return "shield1";

                case "s2":
                    return "shield2";

                case "s3":
                    return "shield3";

                case "s4":
                    return "shield4";

                case "s5":
                    return "shield5";

                case "s6":
                    return "shield6";

                case "s-1":
                    return "shield-1";

                case "s-2":
                    return "shield-2";

                case "s-3":
                    return "shield-3";

                case "s-4":
                    return "shield-4";

                case "s-5":
                    return "shield-5";

                case "s-6":
                    return "shield-6";

                case "double":
                    return "shield-double";
            }
            return act;
        }

        public void PlayMovie(string action, int delay, int movieTime)
        {
            if (this.m_game != null)
            {
                this.m_game.AddAction(new PhysicalObjDoAction(this, action, delay, movieTime));
            }
        }

        public void SetGame(BaseGame game)
        {
            this.m_game = game;
        }

        public Dictionary<string, string> ActionMapping
        {
            get
            {
                return this.m_actionMapping;
            }
        }

        public bool CanPenetrate
        {
            get
            {
                return this.m_canPenetrate;
            }
            set
            {
                this.m_canPenetrate = value;
            }
        }

        public string CurrentAction
        {
            get
            {
                return this.m_currentAction;
            }
            set
            {
                this.m_currentAction = value;
            }
        }

        public string Model
        {
            get
            {
                return this.m_model;
            }
        }

        public string Name
        {
            get
            {
                return this.m_name;
            }
        }

        public virtual int phyBringToFront
        {
            get
            {
                return this.m_phyBringToFront;
            }
        }

        public int Rotation
        {
            get
            {
                return this.m_rotation;
            }
        }

        public int Scale
        {
            get
            {
                return this.m_scale;
            }
        }

        public virtual int Type
        {
            get
            {
                return this.m_type;
            }
        }

        public int typeEffect
        {
            get
            {
                return this.m_typeEffect;
            }
        }
    }
}

