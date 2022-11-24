import React, { useState } from 'react';
import { Spinner, ListGroup, Alert, Button, Badge, FormGroup, Label, Input, Row, Col} from 'reactstrap';
import { useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from 'react-query';
import { fetchThreatModelCategory, createThreatModel } from '../fetchers/threatmodels';
import Category from './Category';
import { useEffect } from 'react';


const AddThreatModel = () => {

    const navigate = useNavigate();

    const { isError, isLoading, data, error } = useQuery(['threatmodel-category'], fetchThreatModelCategory, { staleTime: 24 * 60 * 60 * 1000 });
    const category = data;

    const [selectedList, setSelectedList] = useState([]);
    const [projectName, setProjectName] = useState('');
    const [dataflowAttributes, setDataflowAttributes] = useState([]);

    const queryClient = useQueryClient();

    const createThreatModelMutation = useMutation(threatModel => {
        return createThreatModel(threatModel);
    });

    const getChildrenIds = category => {
        let ids = [category.id];
        if (category.children) {
            category.children.forEach(c => ids = [...ids, ...getChildrenIds(c)]);
        }
        if (category.recommendations) {
            category.recommendations.forEach(r => ids = [...ids, r.id]);
        }
        return ids;
    }

    const toggleSelectability = selectedCategory => {
        const toggledIds = getChildrenIds(selectedCategory);
        if (selectedList.includes(selectedCategory.id)) {
            setSelectedList(selectedList.filter(id => !toggledIds.includes(id)));
        } else {
            setSelectedList([...selectedList, ...toggledIds.filter(id => !selectedList.includes(id))]);
        }
    }

    const isSelected = id => {
        return selectedList.includes(id);
    }

    const getSelectedRecommendations = (category) => {
        if (!category) {
            return [];
        }
        let recommendations = [...category.recommendations.filter(r => isSelected(r.id))];
        category.children.forEach(c => recommendations = [...recommendations, ...getSelectedRecommendations(c)]);
        return recommendations;
    }

    useEffect(() => {
        if (!data) {
            return;
        }
        setSelectedList(getChildrenIds(data));
    }, [data]);

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

    const selectedRecommendations = getSelectedRecommendations(category);
    const selectedRecommendationsCount = selectedRecommendations.length;

    const handleProjectNameChange = (e) => {
        setProjectName(e.target.value);
    };

    const addDataflowAttributeHandler = () => {
        let nextIndex = 1;
        if (dataflowAttributes.length > 0) {
            nextIndex = Math.max(...dataflowAttributes.map(a => a.number)) + 1;
        }
        const newAttribute = {
            number: nextIndex.toString(),
            transport: 'HTTPS/TLS 1.2',
            dataClassification: 'Confidential',
            authentication: 'Azure AD',
            notes: ''
        };
        setDataflowAttributes([...dataflowAttributes, newAttribute]);
    }

    const deleteDataflowAttributeHandler = (index) => {
        const attributes = [...dataflowAttributes];
        attributes.splice(index, 1);
        setDataflowAttributes(attributes);
    }

    const handleDataflowAttributeChange = (e, index) => {
        const { name, value } = e.target;
        const updatedObject = Object.assign({}, dataflowAttributes[index], { [name]: value });
        setDataflowAttributes([
            ...dataflowAttributes.slice(0, index),
            updatedObject,
            ...dataflowAttributes.slice(index + 1)
        ]);
    }

    const saveThreatModelHandler = async () => {
        var threatModel = {
            projectName: projectName,
            dataflowAttributes: dataflowAttributes,
            threats: selectedRecommendations
        };
        console.log(threatModel);
        try {
            await createThreatModelMutation.mutateAsync(threatModel);
            queryClient.invalidateQueries(['threatmodels']);
            queryClient.refetchQueries('threatmodels', { force: true });
        }
        catch { }
    }

    return (
        <>
            <div className="mb-3">
                <Button color="secondary" onClick={() => navigate('/threatmodels')}>Back to threat models</Button>
            </div>
            <FormGroup>
                <Label for="projectName">Project name</Label>
                <Input id="projectName" name="projectName" placeholder="Enter the project name" value={projectName} onChange={handleProjectNameChange} />
            </FormGroup>
            <FormGroup>
                <Label>Data flow attributes</Label>
                <div>
                    <Button color="success" onClick={addDataflowAttributeHandler}>Add attribute</Button>
                    <Row className="mt-3">
                        <Col md={1}>
                            <Label>#</Label>
                        </Col>
                        <Col className="ps-0">
                            <Label>Transport Protocol</Label>
                        </Col>
                        <Col className="ps-0">
                            <Label>Data Classification</Label>
                        </Col>
                        <Col className="ps-0">
                            <Label>Authentication</Label>
                        </Col>
                        <Col md={6} className="ps-0">
                            <Label>Notes</Label>
                        </Col>
                    </Row>
                    {dataflowAttributes.map((a, index) => (
                        <Row key={index} className="mb-1">
                            <Col md={1}>
                                <Input name="number" value={a.number} onChange={(e) => handleDataflowAttributeChange(e, index)} />
                            </Col>
                            <Col className="ps-0">
                                <Input name="transport" value={a.transport} onChange={(e) => handleDataflowAttributeChange(e, index)} />
                            </Col>
                            <Col className="ps-0">
                                <Input type="select" name="dataClassification" value={a.dataClassification} onChange={(e) => handleDataflowAttributeChange(e, index)}>
                                    <option>Sensitive</option>
                                    <option>Confidential</option>
                                    <option>Private</option>
                                    <option>Proprietary</option>
                                    <option>Public</option>
                                </Input>
                            </Col>
                            <Col className="ps-0">
                                <Input name="authentication" value={a.authentication} onChange={(e) => handleDataflowAttributeChange(e, index)} />
                            </Col>
                            <Col md={6} className="ps-0">
                                <Row>
                                    <Col md={11}>
                                        <Input name="notes" value={a.notes} onChange={(e) => handleDataflowAttributeChange(e, index)} />
                                    </Col>
                                    <Col md={1} className="ps-0">
                                        <Button color="danger" outline onClick={() => deleteDataflowAttributeHandler(index)}>X</Button>
                                    </Col>
                                </Row>
                            </Col>
                        </Row>
                    ))
                    }
                </div>
            </FormGroup>
            <FormGroup>
                <Label>Threat properties</Label>
                {!category ? (
                    <p>There are no recommendations</p>
                ) : (
                    <>
                        {selectedRecommendationsCount > 0 ? (
                            <div className="d-flex justify-content-between align-items-center py-2 px-3 border-bottom border-3 border-dark mb-2 bg-super-light">
                                <span>Selected threats <Badge color="primary" className="ms-2 fs-little-smaller">{selectedRecommendationsCount}</Badge></span>
                            </div>
                        ) : null}
                        <ListGroup flush>
                            <Category category={category} isSelected={isSelected} toggleSelectability={toggleSelectability} />
                        </ListGroup>
                    </>
                )}
            </FormGroup>
            <FormGroup className="border-top border-3 border-dark pt-3">
                <Button color="success" onClick={saveThreatModelHandler}>Save threat model</Button>
            </FormGroup>
        </>
    );
};

export default AddThreatModel;