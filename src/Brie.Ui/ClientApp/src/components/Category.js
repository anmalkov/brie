import React from 'react';
import { ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import Recommendation from './Recommendation';
//import { FcFolder } from "react-icons/fc";

const Category = ({ category, level, collapsedList, toggleCollapsibility }) => {

    if (!level) {
        level = 0;
    }

    const getPaddingLeft = () => {
        return level * 30;
    }

    const calculateRecommendationsCount = category => {
        let count = category.recommendations.length;
        category.children.forEach(c => count += calculateRecommendationsCount(c));
        return count;
    }

    const recommendationsCount = calculateRecommendationsCount(category);

    const isCollapsed = collapsedList.includes(category.id);

    return (
        <>
            <ListGroupItem className="d-flex justify-content-between align-items-center" style={{ paddingLeft: getPaddingLeft() + 'px' }} action href="#" tag="a" onClick={(e) => { e.preventDefault(); toggleCollapsibility(category.id); }}>
                <div><Input className="form-check-input me-2" type="checkbox" /> <b>{category.name}</b></div> <Badge>{recommendationsCount}</Badge>
            </ListGroupItem>
            {!isCollapsed ? (
                <>
                    {category.children.map(c => (
                        <Category key={c.id} category={c} level={level + 1} collapsedList={collapsedList} toggleCollapsibility={toggleCollapsibility} />
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