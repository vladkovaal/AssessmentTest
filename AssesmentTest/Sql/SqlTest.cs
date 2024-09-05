using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

public class AccountCondition
{
    public int id { get; set; }
    public byte Flag_NBS { get; set; }
    public string NBS { get; set; }
    public byte Flag_OB22 { get; set; }
    public string OB22 { get; set; }
}

public class SqlConditionBuilder
{
    public string GetSqlCondition(List<AccountCondition> conditions)
    {
        List<string> sqlConditions = new List<string>();

        foreach (var condition in conditions)
        {
            string nbsCondition = "";
            string ob22Condition = "";

            if (condition.Flag_NBS == 1)
            {
                nbsCondition = $"nbs = {condition.NBS}";
            }
            else
            {
                nbsCondition = $"nbs != {condition.NBS}";
            }

            if (condition.Flag_OB22 == 1)
            {
                ob22Condition = $"ob22 = {condition.OB22}";
            }
            else
            {
                ob22Condition = $"ob22 != {condition.OB22}";
            }

            sqlConditions.Add($"({nbsCondition} and {ob22Condition})");
        }

        return "and (" + string.Join(" and ", sqlConditions) + ")";
    }
}

class Program
{
    static void Main()
    {
        List<AccountCondition> conditions = new List<AccountCondition>
        {
            new AccountCondition { id = 1, Flag_NBS = 1, NBS = "2630", Flag_OB22 = 1, OB22 = "12" },
            new AccountCondition { id = 2, Flag_NBS = 1, NBS = "2620", Flag_OB22 = 0, OB22 = "36" }
        };

        SqlConditionBuilder builder = new SqlConditionBuilder();
        string sqlCondition = builder.GetSqlCondition(conditions);

        Console.WriteLine(sqlCondition);
    }
}
