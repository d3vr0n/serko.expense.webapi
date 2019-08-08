using serko.expense.models;

namespace serko.expense.biz
{
    public interface IExpenseManager
    {
        ExpenseModel ValidateAndExtract(string emailContent);
    }
}