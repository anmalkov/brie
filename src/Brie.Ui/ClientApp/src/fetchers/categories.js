export const fetchCategory = async () => {
    const response = await fetch('api/categories');
    const categories = await response.json();
    return categories;
}