import React, { useState } from 'react';
import { ListGroup, ListGroupItem, Badge, Input } from 'reactstrap';
import { FcFile } from "react-icons/fc";
import ReactMarkdown from 'react-markdown'

const Recommendation = ({ recommendation, level }) => {

    const [isOpen, setIsOpen] = useState(false);

    if (!level) {
        level = 0;
    }

    const getPaddingLeft = () => {
        return level * 30;
    }

    const toggleIsOpen = e => {
        e.preventDefault();
        setIsOpen(!isOpen);
    }

    return (
        <>
            <ListGroupItem style={{ paddingLeft: getPaddingLeft() + 'px' }} action href="#" tag="a" onClick={toggleIsOpen}>
                <Input className="form-check-input me-2" type="checkbox" /> <FcFile /> {recommendation.title}
            </ListGroupItem>
            {isOpen ? (
                <ListGroupItem style={{ paddingLeft: getPaddingLeft() + 'px' }} action href="#" tag="a" onClick={toggleIsOpen}>
                    <div className="ps-5"><ReactMarkdown>{recommendation.description}</ReactMarkdown></div>
                </ListGroupItem>
            ) : null}
        </>
    )
}

export default Recommendation;