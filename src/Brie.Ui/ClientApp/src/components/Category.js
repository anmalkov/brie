import React, { useState } from 'react';
import { ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import Recommendation from './Recommendation';
//import { FcFolder } from "react-icons/fc";

const Category = ({ category, level }) => {

    const [isCollapsed, setIsCollapsed] = useState(false);

    if (!level) {
        level = 0;
    }

    const getPaddingLeft = () => {
        return level * 30;
    }

    const calculateRecommendationsCount = (category) => {
        let count = category.recommendations.length;
        category.children.forEach(c => count += calculateRecommendationsCount(c));
        return count;
    }

    const toggleIsCollapsed = e => {
        e.preventDefault();
        setIsCollapsed(!isCollapsed);
    }

    return (
        <>
            <ListGroupItem className="d-flex justify-content-between align-items-center" style={{ paddingLeft: getPaddingLeft() + 'px' }} action href="#" tag="a" onClick={toggleIsCollapsed}>
                <div><Input className="form-check-input me-2" type="checkbox" /> <b>{category.name}</b></div> <Badge>{calculateRecommendationsCount(category)}</Badge>
            </ListGroupItem>
            {!isCollapsed ? (
                <>
                    {category.children.map(c => (
                        <Category key={c.id} category={c} level={level + 1} />
                    ))}
                    {category.recommendations.map(r => (
                        <Recommendation key={r.id} recommendation={r} level={level+1} />
                    ))}
                </>
            ) : null}
        </>
    )
}

export default Category;