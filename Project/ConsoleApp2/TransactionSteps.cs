using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace ConsoleApp2
{
    public static class TransactionSteps 
    {
        private static bool Prepare()
        {
            foreach (var service in TMData.CompleteEnlistedServices)
            {
                try
                {
                    bool iSprepared = service.Prepare();
                    if (!iSprepared)
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                }

            }
            return true;
        }

        private static bool Commit()
        {
            foreach (var service in TMData.CompleteEnlistedServices)
            {
                try
                {
                    bool isCommited = service.Commit();
                    if (!isCommited)
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            }
            return true;
        }

        private static void Rollback()
        {
            foreach (var service in TMData.CompleteEnlistedServices)
            {
                try
                {
                    service.Rollback();
                }
                catch (Exception)
                {
                }

            }
        }
        public static void BeginTransaction() {

            bool isPrepared = Prepare();

            if (!isPrepared)
            {
                Rollback(); 
                return;
            }

            bool isCommited = Commit();

            if (!isCommited)
            {
                Rollback();
                return;
            }
            else 
            {
                // USPJESNO SVI KOMITOVALI
            }
        }
    }
}
