namespace Game.Logic
{
    using Bussiness;
    using log4net;
    using SqlDataProvider.Data;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Reflection;

    public class ExerciseMgr
    {
        private static Dictionary<int, ExerciseInfo> _exercises;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ReaderWriterLock m_lock;
        private static ThreadSafeRandom rand;

        public static ExerciseInfo FindExercise(int Grage)
        {
            if (Grage == 0)
            {
                Grage = 1;
            }
            m_lock.AcquireReaderLock(0x2710);
            try
            {
                if (_exercises.ContainsKey(Grage))
                {
                    return _exercises[Grage];
                }
            }
            catch
            {
            }
            finally
            {
                m_lock.ReleaseReaderLock();
            }
            return null;
        }

        public static int GetExercise(int GP, string type)
        {
            int exerciseL = 0;
            for (int i = 1; i <= GetMaxLevel(); i++)
            {
                if (FindExercise(i).GP >= GP)
                {
                    return exerciseL;
                }
                if (type != null)
                {
                    if (type != "A")
                    {
                        if (type != "AG")
                        {
                            if (type != "D")
                            {
                                if (type != "H")
                                {
                                    if (type == "L")
                                    {
                                        exerciseL = FindExercise(i).ExerciseL;
                                    }
                                }
                                else
                                {
                                    exerciseL = FindExercise(i).ExerciseH;
                                }
                            }
                            else
                            {
                                exerciseL = FindExercise(i).ExerciseD;
                            }
                        }
                        else
                        {
                            exerciseL = FindExercise(i).ExerciseAG;
                        }
                    }
                    else
                    {
                        exerciseL = FindExercise(i).ExerciseA;
                    }
                }
            }
            return exerciseL;
        }

        public static int GetMaxLevel()
        {
            if (_exercises == null)
            {
                Init();
            }
            return _exercises.Values.Count;
        }

        public static bool Init()
        {
            try
            {
                m_lock = new ReaderWriterLock();
                _exercises = new Dictionary<int, ExerciseInfo>();
                rand = new ThreadSafeRandom();
                return LoadExercise(_exercises);
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ExercisesMgr", exception);
                }
                return false;
            }
        }

        private static bool LoadExercise(Dictionary<int, ExerciseInfo> Exercise)
        {
            using (PlayerBussiness bussiness = new PlayerBussiness())
            {
                foreach (ExerciseInfo info in bussiness.GetAllExercise())
                {
                    if (!Exercise.ContainsKey(info.Grage))
                    {
                        Exercise.Add(info.Grage, info);
                    }
                }
            }
            return true;
        }

        public static bool ReLoad()
        {
            try
            {
                Dictionary<int, ExerciseInfo> exercise = new Dictionary<int, ExerciseInfo>();
                if (LoadExercise(exercise))
                {
                    m_lock.AcquireWriterLock(-1);
                    try
                    {
                        _exercises = exercise;
                        return true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_lock.ReleaseWriterLock();
                    }
                }
            }
            catch (Exception exception)
            {
                if (log.IsErrorEnabled)
                {
                    log.Error("ExerciseMgr", exception);
                }
            }
            return false;
        }
    }
}

