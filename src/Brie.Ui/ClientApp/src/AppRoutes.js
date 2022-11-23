import Home from "./components/Home";
import ThreatModels from "./components/ThreatModels";
import AddThreatModel from "./components/AddThreatModel";
import Recommendations from "./components/Recommendations";

const AppRoutes = [
    {
        index: true,
        element: <Home />
    },
    {
        path: '/threatmodels',
        element: <ThreatModels />
    },
    {
        path: '/addthreatmodel',
        element: <AddThreatModel />
    },
    {
        path: '/recommendations',
        element: <Recommendations />
    }  
];

export default AppRoutes;
