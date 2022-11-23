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