export const fetchThreatModels = async () => {
    const response = await fetch('api/threatmodels');
    const result = await response.json();
    if (response.status !== 200) {
        throw Error(result.detail);
    }
    return result;
}

export const fetchThreatModelCategory = async () => {
    const response = await fetch('api/threatmodels/categories');
    const result = await response.json();
    if (response.status !== 200) {
        throw Error(result.detail);
    }
    return result;
}

export const fetchThreatModelReport = async (id) => {
    const response = await fetch(`api/threatmodels/${id}/report`);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
    const report = await response.text();
    return report;
}

export const createThreatModel = async (threatModel) => {
    const request = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(threatModel)
    };
    const response = await fetch('api/threatmodels', request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}

export const deleteThreatModel = async (id) => {
    const request = {
        method: 'DELETE'
    };
    const response = await fetch(`api/threatmodels/${id}`, request);
    if (response.status !== 200) {
        const result = await response.json();
        throw Error(result.detail);
    }
}