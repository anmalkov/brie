import React from 'react';
import { Spinner, ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchCategory } from '../fetchers/categories';
import Category from './Category';

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
                    <Category key={c.name} category={c} />
                ))}
            </ListGroup>
        </div>
    );
};

export default Recommendations;