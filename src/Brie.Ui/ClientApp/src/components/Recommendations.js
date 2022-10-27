import React, { useEffect, useState } from 'react';
import { Spinner, ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import { useQuery } from 'react-query';
import { fetchCategory } from '../fetchers/categories';
import Category from './Category';

const Recommendations = () => {

    const { isError, isLoading, data, error } = useQuery(['category'], fetchCategory);
    const category = data;

    const [selectedList, setSelectedList] = useState([]);

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
                    <Category key={c.id} category={c} isSelected={isSelected} toggleSelectability={toggleSelectability} />
                ))}
            </ListGroup>
        </div>
    );
};

export default Recommendations;