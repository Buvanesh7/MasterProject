using AkryazTools.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Helpers
{
    public static class FilterHelper
    {
        public static bool FilterItems(string rule, string data, string filterText)
        {
            if (string.IsNullOrEmpty(filterText))
                return true;
            if (rule == "Equal To")
            {
                if (data.ToLower() == filterText.ToLower())
                    return true;
                else
                    return false;
            }
            else if (rule == "Not Equal To")
            {
                if (data.ToLower() != filterText.ToLower())
                    return true;
                else
                    return false;
            }
            else if (rule == "Contains")
            {
                if (data.ToLower().Contains(filterText.ToLower()))
                    return true;
                else
                    return false;
            }
            else if (rule == "Does Not Contains")
            {
                if (!(data.ToLower().Contains(filterText.ToLower())))
                    return true;
                else
                    return false;
            }
            else if (rule == "Begins With")
            {
                if (data.ToLower().StartsWith(filterText.ToLower()))
                    return true;
                else
                    return false;
            }
            else if (rule == "Does Not Begins With")
            {
                if (!(data.ToLower().StartsWith(filterText.ToLower())))
                    return true;
                else
                    return false;
            }
            else if (rule == "Ends With")
            {
                if (data.ToLower().EndsWith(filterText.ToLower()))
                    return true;
                else
                    return false;
            }
            else if (rule == "Does Not Ends With")
            {
                if (!(data.ToLower().EndsWith(filterText.ToLower())))
                    return true;
                else
                    return false;
            }                      

            return false;
        }
    }
}
