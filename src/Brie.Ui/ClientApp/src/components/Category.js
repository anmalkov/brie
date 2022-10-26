import React from 'react';
import { ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import Recommendation from './Recommendation';
//import { FcFolder } from "react-icons/fc";

const Category = ({ category, level }) => {

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

    return (
        <>
            <ListGroupItem className="d-flex justify-content-between align-items-center" style={{
                paddingLeft: getPaddingLeft()+'px'
            }} action href="#" tag="a">
                <div><Input className="form-check-input me-2" type="checkbox" /> <b>{category.name}</b></div> <Badge>{recommendationsCount}</Badge>
        </ListGroupItem>
        {category.children.map(c => (
            <Category key={c.name} category={c} level={level+1} />
        ))}
        {category.recommendations.map(r => (
            <Recommendation key={r.name} recommendation={r} level={level+1} />
        ))}
        </>
    )
}

export default Category;