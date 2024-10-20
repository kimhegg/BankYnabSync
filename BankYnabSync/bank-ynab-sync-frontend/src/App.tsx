import React from "react";
import { BankList } from "./components/BankList";
import { TransactionList } from "./components/TransactionList";
import { SyncButton } from "./components/SyncButton";
import GoCardlessFlow from "./components/GoCardlessFlow";

const App: React.FC = () => {
  return (
    <div className="container mt-4">
      <h1 className="text-center mb-4">Bank & YNAB Sync</h1>
      <GoCardlessFlow />
      <hr className="my-4" />
      <BankList onBankSelect={() => {}} />
      <SyncButton />
      <TransactionList bankAccountPath="" />
    </div>
  );
};

export default App;
