import React from 'react';
import { Spinner, Alert, Button, Table } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchThreatModels } from '../fetchers/threatmodels';
import { useNavigate } from 'react-router-dom';

const ThreatModels = () => {

    const navigate = useNavigate();

    const { isError, isLoading, data, error } = useQuery(['threatmodels'], fetchThreatModels, { staleTime: 1 * 60 * 60 * 1000 });
    const threatmodels = data;

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
                <Table hover>
                    <thead>
                        <tr>
                            <th scope="col" className="w-50">Project name</th>
                            <th scope="col">Created</th>
                            <th scope="col">Updated</th>
                            <th scope="col"></th>
                        </tr>
                    </thead>
                    <tbody>
                        {threatmodels.sort((a, b) => a.projectName > b.projectName ? 1 : -1).map(t => (
                            <tr key={t.id}>
                                <td>{t.projectName}</td>
                                <td>{(new Date(t.createdAt)).toLocaleDateString()}</td>
                                <td>{t.updatedAt ? (new Date(t.updatedAt)).toLocaleDateString() : 'Never'}</td>
                                <td><Button size="sm" outline color="primary" onClick={() => navigate('/threatmodelreport', { state: { id: t.id } })}>Show report</Button></td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            )}
        </>
    );
};

export default ThreatModels;