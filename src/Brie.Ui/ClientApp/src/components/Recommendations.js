import React, { useEffect, useState } from 'react';
import { Spinner, ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchCategory } from '../fetchers/categories';
import Category from './Category';

const Recommendations = () => {

    const { isError, isLoading, data, error } = useQuery(['category'], fetchCategory);
    const category = data;

    const [collapsedList, setCollapsedList] = useState([]);

    const toggleCollapsibility = id => {
        if (collapsedList.includes(id)) {
            setCollapsedList(collapsedList.filter(c => c != id));
        } else {
            setCollapsedList([...collapsedList, id]);
        }
    }

    if (isLoading) {
        return (
            <div className="text-center">
                <Spinner>
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
                    <Category key={c.id} category={c} collapsedList={collapsedList} toggleCollapsibility={toggleCollapsibility} />
                ))}
            </ListGroup>
        </div>
    );
};

export default Recommendations;