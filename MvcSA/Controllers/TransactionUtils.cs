using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;

namespace MvcSA.Controllers
{
    public static class TransactionUtils
    {
        public static TransactionScope CreateTransactionScope()
        {
            var transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            transactionOptions.Timeout = TransactionManager.MaximumTimeout;
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions);
        }
    }
}