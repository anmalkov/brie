import React from 'react';
import { Spinner, ListGroup, ListGroupItem, Badge } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchCategory } from '../fetchers/categories';

const Recommendations = () => {

    const { isError, isLoading, data, error } = useQuery(['category'], fetchCategory);
    const category = data

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner color="light">
                    Loading...
                </Spinner>
            </div>
        );
    }

    if (!category) {
        return (
            <div>
                <p>There are no recommendations</p>
            </div>
        );
    }

    return (
        <div>
            <ListGroup flush>
                {category.children.map(c => (
                    <ListGroupItem key={c.name} className="d-flex justify-content-between align-items-center" action href="#" tag="a">
                        <div><input className="form-check-input me-2" type="checkbox" checked={true} /> {c.name}</div> <Badge>13</Badge>
                    </ListGroupItem>
                ))}
            </ListGroup>
        </div>
    );
};

export default Recommendations;