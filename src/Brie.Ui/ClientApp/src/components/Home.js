import React from 'react';
import { Link } from "react-router-dom"

const Home = () => {
    return (
        <div>
            <div className="mb-4">
                <h4>WHAT</h4>
                <p>
                    BRIE stands for Bootstrap Recommendation Inclusion Engine.<br />
                    This tool helps facilitate better security practices during customer engagements, by providing an
                    easy-to-use checklist of recommendations which can be exported into the engineering backlog as Azure
                    DevOps work items or GitHub issues.
                </p>
            </div>
            <div className="mb-4">
                <h4>HOW</h4>
                <p>
                    The process as simple as pressing a button.<br />
                    Go to <Link to="/recommendations">Recommendations</Link> page and select the categories and recommendations 
                    that you want to include into your backlog and then click the Export button.
                </p>
            </div>
        </div>
    );
};

export default Home;