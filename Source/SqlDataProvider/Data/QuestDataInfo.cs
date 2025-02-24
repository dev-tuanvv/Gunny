namespace SqlDataProvider.Data
{
    using System;

    public class QuestDataInfo : DataObject
    {
        private DateTime _completeDate;
        private int _condition1;
        private int _condition2;
        private int _condition3;
        private int _condition4;
        private bool _isComplete;
        private bool _isExist;
        private int _questID;
        private int _randDobule;
        private int _repeatFinish;
        private int _userID;

        public int GetConditionValue(int index)
        {
            switch (index)
            {
                case 0:
                    return this.Condition1;

                case 1:
                    return this.Condition2;

                case 2:
                    return this.Condition3;

                case 3:
                    return this.Condition4;
            }
            throw new Exception("Quest condition index out of range.");
        }

        public void SaveConditionValue(int index, int value)
        {
            switch (index)
            {
                case 0:
                    this.Condition1 = value;
                    break;

                case 1:
                    this.Condition2 = value;
                    break;

                case 2:
                    this.Condition3 = value;
                    break;

                case 3:
                    this.Condition4 = value;
                    break;

                default:
                    throw new Exception("Quest condition index out of range.");
            }
        }

        public DateTime CompletedDate
        {
            get
            {
                return this._completeDate;
            }
            set
            {
                this._completeDate = value;
                base._isDirty = true;
            }
        }

        public int Condition1
        {
            get
            {
                return this._condition1;
            }
            set
            {
                this._condition1 = value;
                base._isDirty = true;
            }
        }

        public int Condition2
        {
            get
            {
                return this._condition2;
            }
            set
            {
                this._condition2 = value;
                base._isDirty = true;
            }
        }

        public int Condition3
        {
            get
            {
                return this._condition3;
            }
            set
            {
                this._condition3 = value;
                base._isDirty = true;
            }
        }

        public int Condition4
        {
            get
            {
                return this._condition4;
            }
            set
            {
                this._condition4 = value;
                base._isDirty = true;
            }
        }

        public bool IsComplete
        {
            get
            {
                return this._isComplete;
            }
            set
            {
                this._isComplete = value;
                base._isDirty = true;
            }
        }

        public bool IsExist
        {
            get
            {
                return this._isExist;
            }
            set
            {
                this._isExist = value;
                base._isDirty = true;
            }
        }

        public int QuestID
        {
            get
            {
                return this._questID;
            }
            set
            {
                this._questID = value;
                base._isDirty = true;
            }
        }

        public int RandDobule
        {
            get
            {
                return this._randDobule;
            }
            set
            {
                this._randDobule = value;
                base._isDirty = true;
            }
        }

        public int RepeatFinish
        {
            get
            {
                return this._repeatFinish;
            }
            set
            {
                this._repeatFinish = value;
                base._isDirty = true;
            }
        }

        public int UserID
        {
            get
            {
                return this._userID;
            }
            set
            {
                this._userID = value;
                base._isDirty = true;
            }
        }
    }
}

