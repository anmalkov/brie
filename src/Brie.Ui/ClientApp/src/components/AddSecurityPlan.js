import React from 'react';
import { Spinner, ListGroup, Alert, Button, Badge, NavLink } from 'reactstrap';
import { useNavigate } from 'react-router-dom';

const AddSecurityPlan = () => {

    const navigate = useNavigate();

    return (
        <>
            <div className="mb-3">
                <Button color="secondary" onClick={() => navigate('/secplans')}>Back to plans</Button>
            </div>
        </>
    );
};

export default AddSecurityPlan;