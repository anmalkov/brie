import Home from "./components/Home";
import SecurityPlans from "./components/SecurityPlans";
import AddSecurityPlan from "./components/AddSecurityPlan";
import Recommendations from "./components/Recommendations";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/secplans',
        element: <SecurityPlans />
    },
    {
        path: '/addplan',
        element: <AddSecurityPlan />
    },
    {
        path: '/recommendations',
        element: <Recommendations />
    }  
];

export default AppRoutes;
