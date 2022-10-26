import Home from "./components/Home";
import Recommendations from "./components/Recommendations";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/recommendations',
        element: <Recommendations />
    }  
];

export default AppRoutes;
