import React, { useState } from "react";
import { api } from "../services/api";

export const SyncButton: React.FC = () => {
  const [syncStatus, setSyncStatus] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const handleSync = async () => {
    try {
      setIsLoading(true);
      setSyncStatus("Syncing...");
      const result = await api.syncTransactions();
      setSyncStatus(result);
    } catch (error) {
      setSyncStatus("Sync failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="card mb-4">
      <div className="card-body">
        <button
          className="btn btn-primary btn-lg btn-block"
          onClick={handleSync}
          disabled={isLoading}
        >
          {isLoading ? (
            <span>
              <span
                className="spinner-border spinner-border-sm mr-2"
                role="status"
                aria-hidden="true"
              ></span>
              Syncing...
            </span>
          ) : (
            "Sync Transactions"
          )}
        </button>
        {syncStatus && (
          <div
            className={`alert mt-3 ${
              syncStatus.includes("failed") ? "alert-danger" : "alert-success"
            }`}
            role="alert"
          >
            {syncStatus}
          </div>
        )}
      </div>
    </div>
  );
};
