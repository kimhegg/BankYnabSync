import axios from 'axios';

const API_BASE_URL = 'http://localhost:5017/api/gocardless';

export interface Institution {
    id: string;
    name: string;
    logo: string;
}

export interface Agreement {
    id: string;
}

export interface Requisition {
    id: string;
    link: string;
}

export interface Transaction {
    transactionId: string;
    debtorName?: string;
    transactionAmount: {
        currency: string;
        amount: string;
    };
    bookingDate: string;
    valueDate: string;
    remittanceInformationUnstructured: string;
}

const goCardlessApi = {
    getInstitutions: async (country: string): Promise<Institution[]> => {
        const response = await axios.get(`${API_BASE_URL}/institutions`, { params: { country } });
        return response.data;
    },

    createAgreement: async (institutionId: string): Promise<Agreement> => {
        const response = await axios.post(`${API_BASE_URL}/agreements`, { institutionId });
        return response.data;
    },

    createRequisition: async (
        institutionId: string,
        redirectUrl: string,
        agreementId: string
    ): Promise<Requisition> => {
        const response = await axios.post(`${API_BASE_URL}/requisitions`, {
            institutionId,
            redirectUrl,
            agreementId,
        });
        return response.data;
    },

    getAccounts: async (requisitionId: string): Promise<string[]> => {
        const response = await axios.get(`${API_BASE_URL}/accounts`, { params: { requisitionId } });
        return response.data.accounts;
    },

    getTransactions: async (accountId: string): Promise<{ booked: Transaction[]; pending: Transaction[] }> => {
        const response = await axios.get(`${API_BASE_URL}/transactions`, { params: { accountId } });
        return response.data.transactions;
    },
};

export default goCardlessApi;