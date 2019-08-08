using System;
using serko.expense.models;

namespace serko.expense.biz
{
    /// <summary>
    /// Manager for Expense related tasks
    /// </summary>
    public class ExpenseManager : IExpenseManager
    {
        public ExpenseModel ValidateAndExtract(string emailContent)
        {
            return new ExpenseModel();
            
        }
    }
}
