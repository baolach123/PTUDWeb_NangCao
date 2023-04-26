import React from "react";
import { Navbar as Nb, Nav } from "react-bootstrap";
import {
    Link
}from 'react-router-dom';

const Navbar =()=>{
    return(
        <Nb collapseOnSelect expand='sm' bg='white' variant="light"
        className="border-bottom shadow">
            <div className="container-fluid">
                <Nb.Brand href='/admin'> Tips & Tricks</Nb.Brand>
                <Nb.Toggle aria-controls="responsive-navbar-nav"/>
                <Nb.Collapse id="responsive-navbar-nav" className="d-sm-inline-flex justify-content-between">

                </Nb.Collapse>
            </div>
        </Nb>
    )
}