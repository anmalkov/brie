import React, { useState } from 'react';
import { Spinner, ListGroup, Alert, Button, Badge, NavLink } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchThreatModels } from '../fetchers/threatmodels';
import { useNavigate } from 'react-router-dom';
//import Category from './Category';
//import { useEffect } from 'react';
//import './Recommendations.css';

const ThreatModels = () => {

    const navigate = useNavigate();
    const { isError, isLoading, threatmodels, error } = useQuery(['threatmodels'], fetchThreatModels, { staleTime: 1 * 60 * 60 * 1000 });

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
                <Button color="success" onClick={() => navigate('/addthreatmodel')}>New threat model</Button>
            </div>
            {!threatmodels || threatmodels.length == 0 ? (
                <p>There are no threat models</p>
            ) : (
                <>
                </>
            )}
        </>
    );
};

export default ThreatModels;