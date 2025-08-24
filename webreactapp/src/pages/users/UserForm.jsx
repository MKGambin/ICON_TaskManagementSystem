import React, { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";

const UserForm = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [error, setError] = useState("");

    useEffect(() => {
        if (id) {
            axios
                .get(`/api/users/update/${id}`)
                .then(res => setEmail(res.data.email))
                .catch(err => setError("Failed to load user"));
        }
    }, [id]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (id) {
                await axios.put(`/api/users/update`, { Identifier: id, Email: email, });
            } else {
                await axios.post(`/api/users/create`, { Email: email, });
            }
            navigate("/users");
        } catch (err) {
            //alert(JSON.stringify(err));
            console.error(err);
            alert("An error occurred while saving the user.");
        }
    };

    return (
        <div>
            <h3>{id ? "Edit User" : "Create User"}</h3>
            {error && <p style={{ color: "red" }}>{error}</p>}

            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: "0.5rem" }}>
                    <label>Email:</label>
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} style={{ marginLeft: "0.5rem" }} required />
                </div>

                <hr />
                <button type="button" onClick={() => navigate(`/users`)} style={{ marginRight: "0.5rem" }}>
                    Back
                </button>
                <button type="submit">
                    {id ? "Update" : "Create"}
                </button>
            </form>
        </div>
    );
};

export default UserForm;
