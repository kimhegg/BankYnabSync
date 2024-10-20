import React, { useState, useEffect } from "react";
import { api, Transaction } from "../services/api";

interface TransactionListProps {
  bankAccountPath: string;
}

export const TransactionList: React.FC<TransactionListProps> = ({
  bankAccountPath,
}) => {
  const [transactions, setTransactions] = useState<Transaction[]>([]);

  useEffect(() => {
    if (bankAccountPath) {
      api.getTransactions(bankAccountPath).then(setTransactions);
    }
  }, [bankAccountPath]);

  return (
    <div className="card">
      <div className="card-header bg-success text-white">
        <h2 className="mb-0">Transactions</h2>
      </div>
      <div className="card-body">
        <div className="table-responsive">
          <table className="table table-striped table-hover">
            <thead className="thead-dark">
              <tr>
                <th>Date</th>
                <th>Payee</th>
                <th>Amount</th>
                <th>Category</th>
              </tr>
            </thead>
            <tbody>
              {transactions.map((transaction) => (
                <tr key={transaction.id}>
                  <td>{new Date(transaction.date).toLocaleDateString()}</td>
                  <td>{transaction.payee}</td>
                  <td
                    className={
                      transaction.amount < 0 ? "text-danger" : "text-success"
                    }
                  >
                    ${Math.abs(transaction.amount).toFixed(2)}
                  </td>
                  <td>{transaction.category || "Uncategorized"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
