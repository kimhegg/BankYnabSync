import React, { useState } from "react";
import { BankList } from "./components/BankList";
import { TransactionList } from "./components/TransactionList";
import { SyncButton } from "./components/SyncButton";

const App: React.FC = () => {
  const [selectedBankAccountPath, setSelectedBankAccountPath] = useState<
    string | null
  >(null);

  return (
    <div className="container mt-4">
      <h1 className="text-center mb-4">Bank & YNAB Sync</h1>
      <BankList onBankSelect={setSelectedBankAccountPath} />
      <SyncButton />
      {selectedBankAccountPath && (
        <TransactionList bankAccountPath={selectedBankAccountPath} />
      )}
    </div>
  );
};

export default App;
