import React, { useState, useEffect } from "react";
import { api, Bank } from "../services/api";

interface BankListProps {
  onBankSelect: (bankAccountPath: string) => void;
}

export const BankList: React.FC<BankListProps> = ({ onBankSelect }) => {
  const [banks, setBanks] = useState<Bank[]>([]);

  useEffect(() => {
    api.getBanks().then(setBanks);
  }, []);

  return (
    <div className="card mb-4">
      <div className="card-header bg-primary text-white">
        <h2 className="mb-0">Select a Bank</h2>
      </div>
      <div className="card-body">
        <div className="row">
          {banks.map((bank) => (
            <div key={bank.id} className="col-md-4 mb-3">
              <button
                className="btn btn-outline-primary w-100 h-100 d-flex flex-column align-items-center justify-content-center"
                onClick={() => onBankSelect(bank.id)}
              >
                <img
                  src={bank.logo}
                  alt={bank.name}
                  className="mb-2"
                  style={{ width: "64px", height: "64px" }}
                />
                <span>{bank.name}</span>
              </button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};
