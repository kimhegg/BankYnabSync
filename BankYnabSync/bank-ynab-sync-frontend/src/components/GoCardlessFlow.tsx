import React, { useState, useEffect } from "react";
import goCardlessApi, {
  Institution,
  Transaction,
} from "../services/goCardlessApi";

const GoCardlessFlow: React.FC = () => {
  const [institutions, setInstitutions] = useState<Institution[]>([]);
  const [selectedInstitution, setSelectedInstitution] =
    useState<Institution | null>(null);
  const [requisitionLink, setRequisitionLink] = useState<string | null>(null);
  const [accounts, setAccounts] = useState<string[]>([]);
  const [transactions, setTransactions] = useState<Transaction[]>([]);

  useEffect(() => {
    const fetchInstitutions = async () => {
      try {
        const institutions = await goCardlessApi.getInstitutions("GB");
        setInstitutions(institutions);
      } catch (error) {
        console.error("Error fetching institutions:", error);
        // Handle the error appropriately (e.g., show an error message to the user)
      }
    };

    fetchInstitutions();
  }, []);

  const handleInstitutionSelect = async (institution: Institution) => {
    try {
      setSelectedInstitution(institution);
      const agreement = await goCardlessApi.createAgreement(institution.id);
      const requisition = await goCardlessApi.createRequisition(
        institution.id,
        "http://localhost:3000/callback", // Replace with your actual callback URL
        agreement.id
      );
      setRequisitionLink(requisition.link);
    } catch (error) {
      console.error("Error creating agreement or requisition:", error);
      // Handle the error appropriately (e.g., show an error message to the user)
    }
  };

  const handleRequisitionComplete = async (requisitionId: string) => {
    try {
      const accountIds = await goCardlessApi.getAccounts(requisitionId);
      setAccounts(accountIds);

      if (accountIds.length > 0) {
        const transactions = await goCardlessApi.getTransactions(accountIds[0]);
        setTransactions(transactions.booked);
      }
    } catch (error) {
      console.error("Error fetching accounts or transactions:", error);
      // Handle the error appropriately (e.g., show an error message to the user)
    }
  };

  return (
    <div className="container mt-4">
      <h2>GoCardless Bank Account Data Flow</h2>
      {!selectedInstitution && (
        <div>
          <h3>Select a Bank</h3>
          <div className="row">
            {institutions.map((institution) => (
              <div key={institution.id} className="col-md-4 mb-3">
                <button
                  className="btn btn-outline-primary w-100"
                  onClick={() => handleInstitutionSelect(institution)}
                >
                  <img
                    src={institution.logo}
                    alt={institution.name}
                    className="mb-2"
                    style={{ width: "64px", height: "64px" }}
                  />
                  <span>{institution.name}</span>
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
      {requisitionLink && (
        <div>
          <h3>Connect to Your Bank</h3>
          <a href={requisitionLink} className="btn btn-primary">
            Connect to {selectedInstitution?.name}
          </a>
        </div>
      )}
      {accounts.length > 0 && (
        <div>
          <h3>Your Accounts</h3>
          <ul>
            {accounts.map((accountId) => (
              <li key={accountId}>{accountId}</li>
            ))}
          </ul>
        </div>
      )}
      {transactions.length > 0 && (
        <div>
          <h3>Recent Transactions</h3>
          <table className="table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Description</th>
                <th>Amount</th>
              </tr>
            </thead>
            <tbody>
              {transactions.map((transaction) => (
                <tr key={transaction.transactionId}>
                  <td>{transaction.bookingDate}</td>
                  <td>{transaction.remittanceInformationUnstructured}</td>
                  <td>{`${transaction.transactionAmount.amount} ${transaction.transactionAmount.currency}`}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default GoCardlessFlow;
