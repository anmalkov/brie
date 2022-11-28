export const fetchThreatModels = async () => {
    const response = await fetch('api/threatmodels');
    const threatmodels = await response.json();
    return threatmodels;
}

export const fetchThreatModelCategory = async () => {
    const response = await fetch('api/threatmodels/categories');
    const categories = await response.json();
    return categories;
}

export const fetchThreatModelReport = async (id) => {
    const response = await fetch(`api/threatmodels/${id}/report`);
    const report = await response.text();
    console.log(report);
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