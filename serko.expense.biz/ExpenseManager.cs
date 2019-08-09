using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using serko.expense.models;

namespace serko.expense.biz
{
    /// <summary>
    /// Manager for Expense related tasks
    /// </summary>
    public class ExpenseManager : IExpenseManager
    {
        private const string ExpenseRegexString = @"<expense>([\s\S]*?)<\/expense>";
        private const string DefaultCostCentre = "UNKNOWN";
        
        /// <summary>
        /// validate and extract xml from email and send parsed result as Expense model
        /// </summary>
        /// <param name="emailContent"></param>
        /// <returns></returns>
        public ExpenseModel ValidateAndExtract(string emailContent)
        {
            Regex expensePattern = new Regex(ExpenseRegexString);
            Match match = expensePattern.Match(emailContent);
            if (match.Success)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ExpenseModel));
                using (var sr = new StringReader(match.Value))
                {
                    var expense = (ExpenseModel)serializer.Deserialize(sr);
                    if (!expense.Total.HasValue)
                    {
                        throw new ArgumentException("Invalid Input. Make sure you have included <total>");
                    }

                    if (string.IsNullOrWhiteSpace(expense.CostCentre))
                    {
                        expense.CostCentre = DefaultCostCentre;
                    }
                    CalculateGST(expense);
                    return expense;
                }
            }
            else
            {
                throw new ArgumentException("Invalid Input. Make sure you have enclosed within <expense></expense>");
            }
        }
        // calculate GST and total excluding GST
        // non pure function
        private void CalculateGST(ExpenseModel expense)
        {
            // we are already asserting Total before calling this method
            // ReSharper disable once PossibleInvalidOperationException
            expense.TotalExcludingGST = Math.Round(expense.Total.Value * 100 / (100 + Constants.GST_PERCENT),2);
            expense.GST = Math.Round((expense.Total.Value - expense.TotalExcludingGST), 2);
        }
    }
}
