import React, { useState } from 'react';
import axios from 'axios';

const CommentForm = ({ lessonId, parentId = null, onCancel = null }) => {
    const [content, setContent] = useState('');
    const [submitting, setSubmitting] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!content.trim()) return;

        setSubmitting(true);
        try {
            const url = parentId ? '/Course/AddReply' : '/Course/AddComment';

            const params = new URLSearchParams();
            params.append('lessonId', lessonId);
            if (parentId) params.append('parentId', parentId);
            params.append('content', content);

            // Add Anti-forgery token if present in DOM
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (token) {
                params.append('__RequestVerificationToken', token);
            }

            console.log("Submitting Q&A to", url, "with params:", params.toString());

            const response = await axios.post(url, params, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            });

            if (response.data.success) {
                setContent('');
                if (onCancel) onCancel();
            }
        } catch (error) {
            console.error("Error submitting comment:", error);
            alert("Đã có lỗi xảy ra.");
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className={parentId ? "mt-3" : "mb-4"}>
            <div className="mb-3">
                <textarea
                    className="form-control"
                    value={content}
                    onChange={(e) => setContent(e.target.value)}
                    rows={parentId ? 2 : 3}
                    placeholder={parentId ? "Trả lời câu hỏi này..." : "Nhập câu hỏi của bạn tại đây..."}
                    style={{ borderRadius: 0, border: '1px solid #1c1d1f', resize: 'none' }}
                    required
                    disabled={submitting}
                ></textarea>
            </div>
            <div className="text-end d-flex justify-content-end gap-2">
                {onCancel && (
                    <button type="button" onClick={onCancel} className="udemy-btn udemy-btn-outline" style={{ padding: '6px 12px', fontSize: '0.85rem' }}>Hủy</button>
                )}
                <button
                    type="submit"
                    className={parentId ? "btn btn-sm btn-dark" : "udemy-btn udemy-btn-primary"}
                    disabled={submitting || !content.trim()}
                >
                    {submitting ? 'Đang gửi...' : (parentId ? 'Gửi trả lời' : 'Gửi câu hỏi')}
                </button>
            </div>
        </form>
    );
};

export default CommentForm;
