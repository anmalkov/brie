export const fetchPlans = async () => {
    const response = await fetch('api/plans');
    const plans = await response.json();
    return plans;
}