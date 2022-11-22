import React, { useState } from 'react';
import { Spinner, ListGroup, Alert, Button, Badge, NavLink } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchPlans } from '../fetchers/plans';
import { useNavigate } from 'react-router-dom';
//import Category from './Category';
//import { useEffect } from 'react';
//import './Recommendations.css';

const SecurityPlans = () => {

    const navigate = useNavigate();
    const { isError, isLoading, plans, error } = useQuery(['plans'], fetchPlans, { staleTime: 1 * 60 * 60 * 1000 });

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner>
                    Loading...
                </Spinner>
            </div>
        );
    }

    if (isError) {
        return (
            <Alert color="danger">{error.message}</Alert >
        );
    }

    return (
        <>
            <div className="mb-3">
                <Button color="success" onClick={() => navigate('/addplan')}>New plan</Button>
            </div>
            {!plans || plans.length == 0 ? (
                <p>There are no security plans</p>
            ) : (
                <>
                </>
            )}
        </>
    );
};

export default SecurityPlans;