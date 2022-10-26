import React from 'react';
import { ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import { FcFile } from "react-icons/fc";

const Recommendation = ({ recommendation, level }) => {

    if (!level) {
        level = 0;
    }

    const getPaddingLeft = () => {
        return level * 30;
    }

    return (
        <>
            <ListGroupItem style={{ paddingLeft: getPaddingLeft() + 'px' }} action href="#" tag="a">
                <Input className="form-check-input me-2" type="checkbox" /> <FcFile /> {recommendation.title}
            </ListGroupItem>
        </>
    )
}

export default Recommendation;