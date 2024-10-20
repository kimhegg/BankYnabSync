import axios from 'axios';

const API_BASE_URL = 'http://localhost:5017/api'; // Adjust this to match your API's URL

export interface Bank {
    id: string;
    name: string;
    logo: string;
}

export interface Transaction {
    id: string;
    date: string;
    amount: number;
    payee: string;
    category: string | null;
}

export const api = {
    getBanks: async (): Promise<Bank[]> => {
        const response = await axios.get(`${API_BASE_URL}/BankYnab/banks`);
        return response.data;
    },

    getTransactions: async (bankAccountPath: string): Promise<Transaction[]> => {
        const response = await axios.get(`${API_BASE_URL}/BankYnab/transactions`, {
            params: { bankAccountPath }
        });
        return response.data;
    },

    syncTransactions: async (): Promise<string> => {
        const response = await axios.get(`${API_BASE_URL}/BankYnab`);
        return response.data;
    }
};