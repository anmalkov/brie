import React from 'react';
import { Spinner, Alert, Button, Table } from 'reactstrap';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import { fetchThreatModels, deleteThreatModel } from '../fetchers/threatmodels';
import { useNavigate } from 'react-router-dom';

const ThreatModels = () => {

    const navigate = useNavigate();

    const { isError, isLoading, data, error } = useQuery(['threatmodels'], fetchThreatModels, { staleTime: 1 * 60 * 60 * 1000 });
    const threatModels = data;

    const queryClient = useQueryClient();

    const deleteThreatModelMutation = useMutation(id => {
        return deleteThreatModel(id);
    });


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

    const getReportUrl = (id) => {
        return `api/threatmodels/${id}/report`;
    }

    const deleteHandler = async (id) => {
        const threatModel = threatModels.find(t => t.id === id);
        if (!threatModel || !window.confirm(`Do you want to delete threat model '${threatModel.projectName}' ?`)) {
            return;
        }
        try {
            await deleteThreatModelMutation.mutateAsync(id);
            queryClient.invalidateQueries(['threatmodels']);
            queryClient.refetchQueries('threatmodels', { force: true });
        }
        catch { }
    }

    return (
        <>
            <div className="mb-3">
                <Button color="success" onClick={() => navigate('/addthreatmodel')}>New threat model</Button>
            </div>
            {!threatModels || threatModels.length === 0 ? (
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
                        {threatModels.sort((a, b) => a.projectName > b.projectName ? 1 : -1).map(t => (
                            <tr key={t.id}>
                                <td>{t.projectName}</td>
                                <td>{(new Date(t.createdAt)).toLocaleDateString()}</td>
                                <td>{t.updatedAt ? (new Date(t.updatedAt)).toLocaleDateString() : 'Never'}</td>
                                <td>
                                    <div className="hstack gap-3 float-end">
                                        <Button size="sm" outline color="success" onClick={() => navigate('/threatmodelreport', { state: { id: t.id } })}>Show report</Button>
                                        <a href={getReportUrl(t.id)} download className="btn btn-outline-success btn-sm">Download</a>
                                        <Button size="sm" outline color="danger" onClick={() => deleteHandler(t.id)}>Delete</Button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            )}
        </>
    );
};

export default ThreatModels;
